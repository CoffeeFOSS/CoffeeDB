import { Component, inject, signal } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { SearchableSignalDefault } from '../../utils/params.utils';
import {
  Column,
  EntityDirectoryComponent,
} from '../entity-directory/entity-directory.component';

@Component({
  selector: 'app-user-directory',
  imports: [EntityDirectoryComponent],
  templateUrl: './user-directory.component.html',
  styleUrl: './user-directory.component.scss',
})
export class UserDirectoryComponent {
  private usersService = inject(UsersService);
  username = signal('');

  signalDefaults: Record<string, SearchableSignalDefault<any>> = {
    u: {
      searchLabel: 'Username',
      signal: this.username,
      defaultValue: undefined,
    },
  };

  columns: Column[] = [
    { header: 'ID', field: 'id' },
    {
      header: 'Username',
      field: 'username',
      link: { key: 'username', type: 'internalId', rootPath: '/users' },
    },
  ];

  fetchUsers = (params: any) =>
    this.usersService.getUsers({
      page: params.p,
      pageSize: params.s,
      username: params.u,
    });
}
