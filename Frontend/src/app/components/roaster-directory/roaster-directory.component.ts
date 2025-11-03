import {
  Component,
  effect,
  inject,
  signal,
  untracked,
  ViewChild,
} from '@angular/core';
import {
  extractAndSetParams,
  resetSearchToSignalDefaults,
  SearchableSignalDefault,
  syncParamsWithUrl,
} from '../../utils/params.utils';
import { RoastersService } from '../../services/roasters.service';
import {
  Column,
  EntityDirectoryComponent,
} from '../entity-directory/entity-directory.component';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthDirective } from '../../directive/auth.directive';
import { QUERY_PARAMS } from '../../constants/query.constants';

@Component({
  selector: 'custom-search',
  template: `
    <ng-content>custom-search</ng-content>
  `,
})
export class CustomSearch {}

@Component({
  selector: 'app-roasters',
  imports: [EntityDirectoryComponent, AuthDirective],
  templateUrl: './roaster-directory.component.html',
  styleUrl: './roaster-directory.component.scss',
})
export class RoasterDirectoryComponent {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  name = signal('');
  locationAddress = signal('');
  longitude = signal<number | null>(null);
  latitude = signal<number | null>(null);
  radius = signal<number | null>(null);
  coordinateSearchEnabled = false;

  @ViewChild('directory') directory!: EntityDirectoryComponent<any>;

  ngAfterViewInit() {
    queueMicrotask(() =>
      syncParamsWithUrl({
        router: this.router,
        route: this.route,
        signalDefaults: {
          ...this.directory.paginationSignalDefaults,
          ...this.signalDefaults,
          ...this.getSignalDefaultsToResetCoordSearch(),
        },
      }),
    );

    if (this.latitude() && this.longitude() && this.radius()) {
      this.coordinateSearchEnabled = true;
      const updatedColumns = [...this.columns()];
      if (!updatedColumns.some((c) => c.header === 'Distance (in KM)')) {
        updatedColumns.push({
          header: 'Distance (kilometers)',
          field: 'distanceInKilometers',
        });
      }
      this.columns.set(updatedColumns);
    }
  }

  constructor() {}

  signalDefaults: Record<string, SearchableSignalDefault<any>> = {
    n: {
      searchLabel: 'Name',
      signal: this.name,
      defaultValue: undefined,
    },
    l: {
      searchLabel: 'Address',
      signal: this.locationAddress,
      defaultValue: undefined,
    },
    la: {
      searchLabel: 'Latitude',
      signal: this.latitude,
      defaultValue: undefined,
      unionId: 'coordinateSearch',
    },
    lo: {
      searchLabel: 'Longitude',
      signal: this.longitude,
      defaultValue: undefined,
      unionId: 'coordinateSearch',
    },
    r: {
      searchLabel: 'Radius',
      signal: this.radius,
      defaultValue: undefined,
      unionId: 'coordinateSearch',
    },
  };

  columns = signal<Column[]>([
    { header: 'ID', field: 'id' },
    {
      header: 'Name',
      field: 'name',
      link: { key: 'id', type: 'internalId', rootPath: '/roasters' },
    },
    { header: 'Address', field: 'locationAddress' },
  ]);

  fetchRoasters = (params: any) =>
    this.roastersService.getRoasters({
      page: params.p,
      pageSize: params.s,
      name: params.n,
      locationAddress: params.l,
      ...(this.coordinateSearchEnabled && {
        lat: params.la,
        long: params.lo,
        radius: params.r,
      }),
    });

  onNavigateCreate() {
    this.router.navigate(['/roasters/create']);
  }

  isSearchEnabled() {
    if (
      this.coordinateSearchEnabled &&
      (!this.latitude() || !this.longitude() || !this.radius())
    ) {
      return false;
    }
    return Object.entries(this.signalDefaults).some(
      ([, { signal, defaultValue }]) => !!signal() && signal() !== defaultValue,
    );
  }

  toggleCoordinateSearch() {
    if (this.coordinateSearchEnabled) {
      this.coordinateSearchEnabled = false;
      this.latitude.set(this.signalDefaults['la'].defaultValue);
      this.longitude.set(this.signalDefaults['lo'].defaultValue);
      this.radius.set(this.signalDefaults['r'].defaultValue);

      const updatedColumns = [...this.columns()];
      const index = updatedColumns.findIndex(
        (c) => c.header === 'Distance (in KM)',
      );
      if (index !== -1) updatedColumns.splice(index, 1);
      this.columns.set(updatedColumns);
    } else {
      this.coordinateSearchEnabled = true;
      const updatedColumns = [...this.columns()];
      if (!updatedColumns.some((c) => c.header === 'Distance (in KM)')) {
        updatedColumns.push({
          header: 'Distance (kilometers)',
          field: 'distanceInKilometers',
        });
      }
      this.columns.set(updatedColumns);
    }
  }

  getSignalDefaultsToResetCoordSearch() {
    return (
      (!this.latitude() || !this.longitude() || !this.radius()) && {
        // this will reset signals
        la: {
          signal: this.latitude,
          defaultValue: this.latitude(),
        },
        lo: {
          signal: this.longitude,
          defaultValue: this.longitude(),
        },
        r: {
          signal: this.radius,
          defaultValue: this.radius(),
        },
      }
    );
  }

  onSearch() {
    if (!this.isSearchEnabled()) return;
    this.directory.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    this.directory.onRefreshData();
  }

  onResetSearch() {
    resetSearchToSignalDefaults(this.directory.signalDefaults());
    this.directory.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    this.directory.onRefreshData();
  }
}
