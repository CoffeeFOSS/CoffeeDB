import { Component, effect, inject, signal, untracked } from '@angular/core';
import { HotToastService } from '@ngxpert/hot-toast';
import { QUERY_PARAMS } from '../../../constants/query.constants';
import { PaginatedResult } from '../../../models/pagination';
import { LoadingService } from '../../../services/loading.service';
import {
  getPaginatedResult,
  getPaginationText,
} from '../../../utils/pagination.utils';
import { SimpleModalComponent } from '../../modal/modal.component';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';
import { RoastersService } from '../../../services/roasters.service';
import { Roaster } from '../../../models/roaster';
import { Router, RouterLink } from '@angular/router';
import { resetSearchToSignalDefaults } from '../../../utils/params.utils';

@Component({
  selector: 'app-roaster-manager',
  imports: [SimpleModalComponent, PaginationControlsComponent, RouterLink],
  templateUrl: './roaster-manager.component.html',
  styleUrl: './roaster-manager.component.scss',
})
export class RoasterManagerComponent {
  private toast = inject(HotToastService);
  private router = inject(Router);
  private roastersService = inject(RoastersService);
  loadingService = inject(LoadingService);

  // Dont store cache since having the most updated info is important
  paginatedResult: PaginatedResult<Roaster[]> | null = null;
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  name = signal('');
  locationAddress = signal('');
  selectedRoaster: Roaster | null = null;
  deletedRoasterIds = new Set<number>();

  private signalDefaults = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
    n: { signal: this.name, defaultValue: undefined },
    l: { signal: this.locationAddress, defaultValue: undefined },
  };

  constructor() {
    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => {
        this.fetchRoasters();
      });
    });
  }

  fetchRoasters() {
    this.loadingService.busy('roaster-manager');
    this.roastersService
      .getRoasters({
        page: this.page(),
        pageSize: this.pageSize(),
        name: this.name(),
        address: this.locationAddress(),
      })
      .subscribe({
        next: (response) => {
          this.loadingService.idle('roaster-manager');
          this.paginatedResult = getPaginatedResult(response);
        },
        error: (error) => {
          console.error(error);
          this.loadingService.idle('roaster-manager');
        },
      });
  }

  get paginationText(): string {
    return getPaginationText(this.paginatedResult);
  }

  showModal(roaster: Roaster) {
    this.selectedRoaster = { ...roaster }; // shallow copy to avoid messing up the original
  }

  hideModal() {
    this.selectedRoaster = null;
  }

  onDeleteRoaster() {
    if (!this.selectedRoaster) return;
    const { name, id } = this.selectedRoaster;
    const loadingId = `delete-roaster-${id}`;
    this.loadingService.busy(loadingId);

    this.roastersService.onDeleteRoaster(id).subscribe({
      next: () => {
        const deletedRoaster = this.paginatedResult?.items?.find(
          (r: Roaster) => r.id === id,
        );
        if (!deletedRoaster) {
          this.toast.success(
            `Roaster ${name} not found in client side memory, this should not happen`,
          );
          return;
        }
        this.deletedRoasterIds.add(id);
        this.toast.success(`Roaster '${name}' deleted`);
        this.loadingService.idle(loadingId);
        this.hideModal();
      },
      error: (error) => {
        console.error(error);
        this.toast.error(error);
        this.loadingService.idle(loadingId);
        this.hideModal();
      },
    });
  }

  onEditNavigate(id: number) {
    this.router.navigate(['/roasters/edit', id]);
  }

  onChangeName(event: Event) {
    this.name.set((event.target as HTMLInputElement).value);
  }

  onChangeLocation(event: Event) {
    this.locationAddress.set((event.target as HTMLInputElement).value);
  }

  onSearchRoaster() {
    if (!this.name() && !this.locationAddress()) return;
    this.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    this.fetchRoasters();
  }

  onResetSearch() {
    resetSearchToSignalDefaults(this.signalDefaults);
    this.fetchRoasters();
  }
}
