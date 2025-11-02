import { Component, inject, signal } from '@angular/core';
import { SearchableSignalDefault } from '../../utils/params.utils';
import { RoastersService } from '../../services/roasters.service';
import {
  Column,
  EntityDirectoryComponent,
} from '../entity-directory/entity-directory.component';
import { Router } from '@angular/router';
import { AuthDirective } from '../../directive/auth.directive';

@Component({
  selector: 'app-roasters',
  imports: [EntityDirectoryComponent, AuthDirective],
  templateUrl: './roaster-directory.component.html',
  styleUrl: './roaster-directory.component.scss',
})
export class RoasterDirectoryComponent {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  name = signal('');
  locationAddress = signal('');

  signalDefaults: Record<string, SearchableSignalDefault<any>> = {
    n: {
      searchLabel: 'Name',
      signal: this.name,
      defaultValue: undefined,
    },
    l: {
      searchLabel: 'Location Address',
      signal: this.locationAddress,
      defaultValue: undefined,
    },
  };

  columns: Column[] = [
    { header: 'ID', field: 'id' },
    {
      header: 'Name',
      field: 'name',
      link: { key: 'id', type: 'internalId', rootPath: '/roasters' },
    },
    { header: 'Location Address', field: 'locationAddress' },
  ];

  fetchRoasters = (params: any) =>
    this.roastersService.getRoasters({
      page: params.p,
      pageSize: params.s,
      name: params.n,
      locationAddress: params.l,
    });

  onNavigateCreate() {
    this.router.navigate(['/roasters/create']);
  }
}
