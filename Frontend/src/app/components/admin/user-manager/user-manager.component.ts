import { Component, effect, inject, signal } from '@angular/core';
import { AdminService } from '../../../services/admin.service';
import { User } from '../../../models/user';
import { getPaginatedResult } from '../../../utils/pagination.utils';
import { PaginatedResult } from '../../../models/pagination';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';
import { SimpleModalComponent } from '../../modal/modal.component';

@Component({
  selector: 'app-user-manager',
  imports: [PaginationControlsComponent, SimpleModalComponent],
  templateUrl: './user-manager.component.html',
  styleUrl: './user-manager.component.scss',
})
export class UserManagerComponent {
  private adminService = inject(AdminService);

  // We are allowing page size change on this component, so we will not be storing
  // pulled paginated data as a cache in a signal, unlike users.service.ts
  paginatedResult: PaginatedResult<User[]> | null = null;
  page = signal(1);
  pageSize = signal(10);
  selectedUser: User | null = null;

  fetchUsersEffect = effect(() => {
    this.adminService.getUserWithRoles(this.page(), this.pageSize()).subscribe({
      next: (response) => {
        this.paginatedResult = getPaginatedResult(response);
      },
    });
  });

  get paginationText(): string {
    const pagination = this.paginatedResult?.pagination;
    if (!pagination) return '';

    const { itemsPerPage, currentPage, totalItems } = pagination;
    const start = itemsPerPage * (currentPage - 1) + 1;
    const end = Math.min(itemsPerPage * currentPage, totalItems);

    return `${start}-${end} of ${pagination.totalItems}`;
  }

  showModal(user: User) {
    this.selectedUser = user;
  }

  hideModal() {
    this.selectedUser = null;
  }
}
