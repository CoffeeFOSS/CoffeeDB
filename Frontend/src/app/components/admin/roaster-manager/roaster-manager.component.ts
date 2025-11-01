import { Component, effect, inject, signal } from '@angular/core';
import { HotToastService } from '@ngxpert/hot-toast';
import { QUERY_PARAMS } from '../../../constants/query.constants';
import { PaginatedResult } from '../../../models/pagination';
import { AccountService } from '../../../services/account.service';
import { LoadingService } from '../../../services/loading.service';
import { getPaginatedResult } from '../../../utils/pagination.utils';
import { SimpleModalComponent } from '../../modal/modal.component';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';
import { RoastersService } from '../../../services/roasters.service';
import { Roaster } from '../../../models/roaster';

@Component({
  selector: 'app-roaster-manager',
  imports: [SimpleModalComponent, PaginationControlsComponent],
  templateUrl: './roaster-manager.component.html',
  styleUrl: './roaster-manager.component.scss',
})
export class RoasterManagerComponent {
  private toast = inject(HotToastService);
  private roastersService = inject(RoastersService);
  private accountService = inject(AccountService);
  loadingService = inject(LoadingService);

  // Dont store cache since having the most updated info is important
  paginatedResult: PaginatedResult<Roaster[]> | null = null;
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  selectedRoaster: Roaster | null = null;

  availableRoles: string[] = ['Admin', 'Moderator'];

  constructor() {
    effect(() => {
      this.loadingService.busy('roaster-manager');
      this.roastersService
        .getRoasters({
          page: this.page(),
          pageSize: this.pageSize(),
          name: undefined, // TODO
          location: undefined, // TODO
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
    });
  }

  get paginationText(): string {
    const pagination = this.paginatedResult?.pagination;
    if (!pagination) return '';

    const { itemsPerPage, currentPage, totalItems } = pagination;
    const start = itemsPerPage * (currentPage - 1) + 1;
    const end = Math.min(itemsPerPage * currentPage, totalItems);

    return `${start}-${end} of ${pagination.totalItems}`;
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
        const deletedUser = this.paginatedResult?.items?.find(
          (r: Roaster) => r.id === id,
        );
        if (!deletedUser) {
          this.toast.success(
            `Roaster ${name} not found in client side memory, this should not happen`,
          );
          return;
        }
        deletedUser.name = '<deleted>';
        deletedUser.alias = '<deleted>';
        deletedUser.location = '<deleted>';
        deletedUser.websiteUrl = '<deleted>';
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
}
