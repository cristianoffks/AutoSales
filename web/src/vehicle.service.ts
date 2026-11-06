import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Vehicle, VehicleInput } from './vehicle';

@Injectable({ providedIn: 'root' })
export class VehicleService {
  private readonly http = inject(HttpClient);
  private readonly url = '/api/vehicles';

  list(params: HttpParams) {
    return this.http.get<Vehicle[]>(this.url, { params });
  }

  get(id: number) {
    return this.http.get<Vehicle>(`${this.url}/${id}`);
  }

  create(vehicle: VehicleInput) {
    return this.http.post<Vehicle>(this.url, vehicle);
  }

  update(id: number, vehicle: VehicleInput) {
    const { plate, manufactureYear, brand, model } = vehicle;
    return this.http.put<void>(`${this.url}/${id}`, { plate, manufactureYear, brand, model });
  }

}
