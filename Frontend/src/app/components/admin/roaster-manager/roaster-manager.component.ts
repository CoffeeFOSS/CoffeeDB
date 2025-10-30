import { Component, effect, inject, signal } from '@angular/core';
import { HotToastService } from '@ngxpert/hot-toast';
import { QUERY_PARAMS } from '../../../constants/query.constants';
import { PaginatedResult } from '../../../models/pagination';
import { UserWithRoles, User } from '../../../models/user';
import { AccountService } from '../../../services/account.service';
import { AdminService } from '../../../services/admin.service';
import { LoadingService } from '../../../services/loading.service';
import { getPaginatedResult } from '../../../utils/pagination.utils';
import { SimpleModalComponent } from '../../modal/modal.component';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';

@Component({
  selector: 'app-roaster-manager',
  imports: [SimpleModalComponent, PaginationControlsComponent],
  templateUrl: './roaster-manager.component.html',
  styleUrl: './roaster-manager.component.scss',
})
export class RoasterManagerComponent {
  private toast = inject(HotToastService);
  private adminService = inject(AdminService);
  accountService = inject(AccountService);
  loadingService = inject(LoadingService);

  // We are allowing page size change on this component, so we will not be storing
  // pulled paginated data as a cache in a signal, unlike users.service.ts
  paginatedResult: PaginatedResult<UserWithRoles[]> | null = null;
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  selectedUser: UserWithRoles | null = null;

  availableRoles: string[] = ['Admin', 'Moderator'];

  fetchUsersEffect = effect(() => {
    this.loadingService.busy('user-manager');
    this.adminService.getUserWithRoles(this.page(), this.pageSize()).subscribe({
      next: (response) => {
        this.loadingService.idle('user-manager');
        this.paginatedResult = getPaginatedResult(response);
      },
      error: (error) => {
        this.loadingService.idle('user-manager');
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
