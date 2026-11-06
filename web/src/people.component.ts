import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PeopleService, Person } from './people.service';
import { readApiError } from './api-error';

@Component({
  changeDetection: ChangeDetectionStrategy.Eager,
  selector: 'app-people',
  imports: [FormsModule, RouterLink],
  templateUrl: './people.component.html',
})
export class PeopleComponent {
  private readonly service = inject(PeopleService);

  name = '';
  cpf = '';
  people: Person[] = [];
  loading = false;
  error = '';
  searched = false;

  get hasFilters(): boolean {
    return !!(this.name.trim() || this.cpf.trim());
  }

  search(): void {
    if (this.loading || !this.hasFilters) {
      return;
    }

    this.searched = true;
    this.loading = true;
    this.error = '';

    this.service.list(this.name, this.cpf).subscribe({
      next: people => {
        this.people = people;
        this.loading = false;
      },
      error: response => {
        this.people = [];
        this.error = readApiError(response).message;
        this.loading = false;
      },
    });
  }

  clear(): void {
    this.name = '';
    this.cpf = '';
    this.people = [];
    this.error = '';
    this.searched = false;
  }
}
