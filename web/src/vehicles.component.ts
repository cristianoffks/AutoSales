import { Location } from '@angular/common';
import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PeopleService } from './people.service';
import { Vehicle } from './vehicle';
import { VehicleService } from './vehicle.service';
import { readApiError } from './api-error';

@Component({
  changeDetection: ChangeDetectionStrategy.Eager,
  selector: 'app-vehicles',
  imports: [FormsModule, RouterLink],
  templateUrl: './vehicles.component.html',
})
export class VehiclesComponent implements OnInit {
  private readonly vehicleService = inject(VehicleService);
  private readonly peopleService = inject(PeopleService);
  private readonly location = inject(Location);
  readonly byOwner = inject(ActivatedRoute).snapshot.data['byOwner'] === true;

  term = '';
  plate = '';
  cpf = '';
  ownerName = '';
  vehicles: Vehicle[] = [];
  loading = false;
  error = '';

  searched = false;

  ngOnInit(): void {
    const { ownerCpf } = this.location.getState() as { ownerCpf?: string };
    if (this.byOwner && ownerCpf) {
      this.cpf = ownerCpf;
      this.search();
    }
  }

  get hasFilters(): boolean {
    return this.byOwner ? !!this.cpf.trim() : !!(this.term.trim() || this.plate.trim());
  }

  search(): void {
    if (this.loading || !this.hasFilters) {
      return;
    }

    this.searched = true;
    this.error = '';
    this.ownerName = '';
    this.loading = true;

    if (this.byOwner) {
      this.searchByOwner();
      return;
    }

    let params = new HttpParams();
    if (this.term.trim()) {
      params = params.set('search', this.term.trim());
    }
    if (this.plate.trim()) {
      params = params.set('plate', this.plate.trim());
    }

    this.loadVehicles(params);
  }

  private searchByOwner(): void {
    const cpf = this.cpf.trim();
    if (!/^(?:[0-9]{11}|[0-9]{3}\.[0-9]{3}\.[0-9]{3}-[0-9]{2})$/.test(cpf)) {
      this.error = 'Informe um CPF com 11 dígitos, com ou sem máscara.';
      this.loading = false;
      return;
    }

    this.peopleService.list('', cpf).subscribe({
      next: ([person]) => {
        if (!person) {
          this.error = 'Nenhuma pessoa encontrada com esse CPF.';
          this.loading = false;
          return;
        }

        this.ownerName = person.fullName;
        this.loadVehicles(new HttpParams().set('personId', person.id), person.id);
      },
      error: response => {
        this.error = readApiError(response).message;
        this.loading = false;
      },
    });
  }

  private loadVehicles(params: HttpParams, personId?: number): void {
    this.vehicleService.list(params).subscribe({
      next: vehicles => {
        (this.vehicles = personId === undefined
          ? vehicles
          : vehicles.filter(vehicle => vehicle.personId === personId));
        this.loading = false;
      },
      error: response => {
        this.error = readApiError(response).message;
        this.loading = false;
      },
    });
  }

  clear(): void {
    this.term = '';
    this.plate = '';
    this.cpf = '';
    this.resetResults();
  }

  resetResults(): void {
    this.ownerName = '';
    this.vehicles = [];
    this.error = '';
    this.searched = false;
  }
}
