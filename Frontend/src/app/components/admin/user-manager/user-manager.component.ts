import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../services/admin.service';
import { User } from '../../../models/user';
import { getPaginatedResult } from '../../../utils/pagination.utils';
import { PaginatedResult } from '../../../models/pagination';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';

@Component({
  selector: 'app-user-manager',
  imports: [PaginationControlsComponent],
  templateUrl: './user-manager.component.html',
  styleUrl: './user-manager.component.scss',
})
export class UserManagerComponent implements OnInit {
  private adminService = inject(AdminService);
  users: User[] = [];

  // This is different from users.service.ts implementation which uses a signal
  // for paginatedResult because we want the most up to date data for admin,
  // and having the variable in this component only makes the signal unnecessary.
  paginatedResult: PaginatedResult<User[]> | null = null;
  page = 1;
  pageSize = 5;
  pageSizeInput = 5; // is used as a temporary pageSize

  ngOnInit(): void {
    this.loadUsersWithRoles();
  }

  loadUsersWithRoles() {
    this.adminService.getUserWithRoles(this.page, this.pageSize).subscribe({
      next: (response) => {
        this.paginatedResult = getPaginatedResult(response);
        this.users = response.body as User[];
      },
    });
  }

  onPageChange(newPage: number) {
    if (
      newPage < 1 ||
      newPage > this.paginatedResult?.pagination?.totalPages!
    ) {
      return;
    }
    this.page = newPage;
    this.loadUsersWithRoles();
  }

  onPageSizeInput($event: Event) {
    this.pageSizeInput = Number(($event.target as HTMLInputElement).value);
  }

  onPageSizeChange($event: number) {
    this.pageSize = $event;
    this.page = 1;
    // this.loadUsersWithRoles();
  }

  get paginationText(): string {
    const pagination = this.paginatedResult?.pagination;
    if (!pagination) return '';

    const { itemsPerPage, currentPage, totalItems } = pagination;
    const start = itemsPerPage * (currentPage - 1) + 1;
    const end = Math.min(itemsPerPage * currentPage, totalItems);

    return `${start}-${end} of ${pagination.totalItems}`;
  }

  onPageUpdate(newPage: number) {
    this.page = newPage;
  }
}
