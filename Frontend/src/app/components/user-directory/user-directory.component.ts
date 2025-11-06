import { Component, inject, signal } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { Member } from '../../models/member';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { FormKeyMap, sanitizeObjectFields } from '../../utils/params.utils';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { PaginatedDirectoryComponent } from '../abstract/paginated-directory/paginated-directory.component';

@Component({
  selector: 'app-user-directory',
  templateUrl: './user-directory.component.html',
  imports: [
    PaginationControlsComponent,
    ReactiveFormsModule,
    TextInputComponent,
  ],
  styleUrls: [
    '../abstract/paginated-directory/paginated-directory.component.scss',
    './user-directory.component.scss',
  ],
})
export class UserDirectoryComponent extends PaginatedDirectoryComponent<
  Member,
  UsersService
> {
  protected service = inject(UsersService);
  protected items = signal<Member[]>([]);
  protected loadingKey = 'user-directory';
  protected formKeyMap: FormKeyMap = {
    username: {
      paramCode: 'u',
      default: '',
      validators: [Validators.maxLength(20)],
    },
  };

  fetchPaginatedItems() {
    return this.service.getUsers({
      page: this.paginationSignals.page.signal(),
      pageSize: this.paginationSignals.pageSize.signal(),
      ...sanitizeObjectFields(this.searchForm.value),
    });
  }
}
