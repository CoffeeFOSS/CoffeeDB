import { Component, inject, signal } from '@angular/core';
import { AdminService } from '../../../services/admin.service';
import { User, UserWithRoles } from '../../../models/user';
import { PaginatedResult } from '../../../models/pagination';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';
import { SimpleModalComponent } from '../../modal/modal.component';
import { AccountService } from '../../../services/account.service';
import { HotToastService } from '@ngxpert/hot-toast';
import { UserDirectoryComponent } from '../../user-directory/user-directory.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TextInputComponent } from '../../forms/text-input/text-input.component';

@Component({
  selector: 'app-user-manager',
  imports: [
    PaginationControlsComponent,
    SimpleModalComponent,
    ReactiveFormsModule,
    TextInputComponent,
  ],
  templateUrl: './user-manager.component.html',
  styleUrls: [
    '../../abstract/paginated-directory/paginated-directory.component.scss',
    './user-manager.component.scss',
  ],
})
export class UserManagerComponent extends UserDirectoryComponent {
  private toast = inject(HotToastService);
  private adminService = inject(AdminService);
  accountService = inject(AccountService);
  selectedUser: UserWithRoles | null = null;
  override items = signal<UserWithRoles[]>([]);
  override cache: Record<string, PaginatedResult<UserWithRoles[]>> = {};
  override loadingKey = 'user-manager';
  override paginatedResultSignal = signal<PaginatedResult<
    UserWithRoles[]
  > | null>(null);

  availableRoles: string[] = ['Admin', 'Moderator'];

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
        const updatedUser = this.paginatedResultSignal()?.items?.find(
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

  override fetchPaginatedItems = () =>
    this.adminService.getUserWithRoles(
      this.paginationSignals.page.signal(),
      this.paginationSignals.pageSize.signal(),
      this.searchForm.value.username || undefined,
    );

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
