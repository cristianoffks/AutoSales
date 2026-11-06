import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize, map, switchMap } from 'rxjs';
import { PeopleService, Person } from './people.service';
import { VehicleService } from './vehicle.service';
import { Vehicle } from './vehicle';
import { readApiError } from './api-error';
@Component({
  changeDetection: ChangeDetectionStrategy.Eager,
  selector: 'app-vehicles',
  imports: [FormsModule, RouterLink],
  templateUrl: './vehicles.component.html',
})
export class VehiclesComponent implements OnInit {
  private readonly peopleService = inject(PeopleService);
  private readonly vehicleService = inject(VehicleService);
  name = ''; cpf = ''; plate = ''; brand = ''; model = ''; year = ''; id = ''; personId = '';
  rows: { vehicle: Vehicle; owner: Person }[] = [];
  loading = false; deleting = false; error = '';
  ngOnInit(): void { this.search(); }
  search(): void {
    if (this.loading || this.deleting) return;
    this.loading = true;
    this.error = '';
    const brand = this.brand.trim().toLocaleLowerCase();
    const model = this.model.trim().toLocaleLowerCase();
    const year = this.year.trim();
    const id = this.id.trim();
    const personId = this.personId.trim();
    let params = new HttpParams();
    if (this.plate.trim()) params = params.set('plate', this.plate.trim());
    if (brand || model) params = params.set('search', this.brand.trim() || this.model.trim());
    this.peopleService.list(this.name, this.cpf).pipe(
      switchMap(people => {
        const owners = people.filter(owner => !personId || owner.id === Number(personId));
        return this.vehicleService.listByOwners(owners, params);
      }),
      map(groups => groups.flatMap(({ person, vehicles }) => vehicles
        .filter(vehicle => vehicle.brand.toLocaleLowerCase().includes(brand) &&
          vehicle.model.toLocaleLowerCase().includes(model) &&
          (!year || vehicle.manufactureYear === Number(year)) && (!id || vehicle.id === Number(id)))
        .map(vehicle => ({ vehicle, owner: person })))),
      finalize(() => this.loading = false),
    ).subscribe({
      next: rows => this.rows = rows,
      error: response => this.error = readApiError(response).message,
    });
  }
  clear(): void {
    this.name = this.cpf = this.plate = this.brand = this.model = this.year = this.id = this.personId = '';
    this.search();
  }
  deleteVehicle(vehicle: Vehicle): void {
    if (this.deleting || !window.confirm(`Excluir o veículo ${vehicle.plate}?`)) return;
    this.deleting = true;
    this.error = '';
    this.vehicleService.delete(vehicle.id).subscribe({
      next: () => { this.rows = this.rows.filter(row => row.vehicle.id !== vehicle.id); this.deleting = false; },
      error: response => { this.error = readApiError(response).message; this.deleting = false; },
    });
  }
}
