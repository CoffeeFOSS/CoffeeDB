import { Component, effect, inject, signal } from '@angular/core';
import { AdminService } from '../../../services/admin.service';
import { User, UserWithRoles } from '../../../models/user';
import { getPaginatedResult } from '../../../utils/pagination.utils';
import { PaginatedResult } from '../../../models/pagination';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';
import { SimpleModalComponent } from '../../modal/modal.component';
import { AccountService } from '../../../services/account.service';
import { HotToastService } from '@ngxpert/hot-toast';
import { LoadingService } from '../../../services/loading.service';

@Component({
  selector: 'app-user-manager',
  imports: [PaginationControlsComponent, SimpleModalComponent],
  templateUrl: './user-manager.component.html',
  styleUrl: './user-manager.component.scss',
})
export class UserManagerComponent {
  private toast = inject(HotToastService);
  private adminService = inject(AdminService);
  accountService = inject(AccountService);
  loadingService = inject(LoadingService);

  // We are allowing page size change on this component, so we will not be storing
  // pulled paginated data as a cache in a signal, unlike users.service.ts
  paginatedResult: PaginatedResult<UserWithRoles[]> | null = null;
  page = signal(1);
  pageSize = signal(10);
  selectedUser: UserWithRoles | null = null;

  availableRoles: string[] = ['Admin', 'Moderator'];

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

  showModal(user: UserWithRoles) {
    this.selectedUser = { ...user }; // shallow copy to avoid messing up the original user
  }

  hideModal() {
    this.selectedUser = null;
  }

  onSubmitRoleEdit() {
    if (!this.selectedUser) return;
    const { username, roles } = this.selectedUser;
    const loadingId = `edit-role-${username}`;
    this.loadingService.busy(loadingId);

    this.adminService.editUserRoles(username, roles).subscribe({
      next: (newRoles: string[]) => {
        const updatedUser = this.paginatedResult?.items?.find(
          (u: User) => u.username === username,
        );
        if (!updatedUser) {
          this.toast.error(`${username} does not exist`);
          return;
        }
        updatedUser.roles = newRoles;
        this.toast.success(`Roles modified for ${username}`);
        this.loadingService.idle(loadingId);
        this.hideModal();
      },
      error: (error) => {
        this.toast.error(error);
        this.loadingService.idle(loadingId);
        this.hideModal();
      },
    });
  }

  updateChecked(value: string) {
    if (!this.selectedUser) return;

    if (this.selectedUser.roles.includes(value)) {
      this.selectedUser.roles = this.selectedUser.roles.filter(
        (r) => r !== value,
      );
    } else {
      this.selectedUser.roles.push(value);
    }
  }

  isAllowedToEditUser(role: string) {
    if (!this.selectedUser) return false;
    if (
      role === 'Admin' &&
      this.selectedUser.username === this.accountService.currentUser()?.username
    )
      return false;
    return true;
  }
}
