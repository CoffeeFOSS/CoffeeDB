import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../services/admin.service';
import { User } from '../../../models/user';
import { getPaginatedResult } from '../../../utils/pagination.utils';
import { PaginatedResult } from '../../../models/pagination';

@Component({
  selector: 'app-user-manager',
  imports: [],
  templateUrl: './user-manager.component.html',
  styleUrl: './user-manager.component.scss',
})
export class UserManagerComponent implements OnInit {
  private adminService = inject(AdminService);
  users: User[] = [];
  paginatedResult = signal<PaginatedResult<User[]> | null>(null);
  page = 1;
  pageSize = 5;
  pageSizeInput = 5; // is used as a temporary pageSize

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUserWithRoles(this.page, this.pageSize).subscribe({
      next: (response) => {
        this.paginatedResult.set(getPaginatedResult(response));
        this.users = response.body as User[];
      },
    });
  }

  onPageChange(newPage: number) {
    if (
      newPage < 1 ||
      newPage > this.paginatedResult()?.pagination?.totalPages!
    ) {
      return;
    }
    this.page = newPage;
    this.getUsersWithRoles();
  }

  onPageSizeInput($event: Event) {
    this.pageSizeInput = Number(($event.target as HTMLInputElement).value);
  }

  onPageSizeChange() {
    this.pageSize = this.pageSizeInput;
    this.page = 1;
    this.getUsersWithRoles();
  }

  get paginationText(): string {
    const pagination = this.paginatedResult()?.pagination;
    if (!pagination) return '';

    const { itemsPerPage, currentPage, totalItems } = pagination;
    const start = itemsPerPage * (currentPage - 1) + 1;
    const end = Math.min(itemsPerPage * currentPage, totalItems);

    return `${start}-${end} of ${pagination.totalItems}`;
  }
}
