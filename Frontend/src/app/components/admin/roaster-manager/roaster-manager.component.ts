import { Component, inject } from '@angular/core';
import { RoasterDirectoryComponent } from '../../roaster-directory/roaster-directory.component';
import { TextInputComponent } from '../../forms/text-input/text-input.component';
import { PaginationControlsComponent } from '../../pagination-controls/pagination-controls.component';
import { ReactiveFormsModule } from '@angular/forms';
import { ErrorTextComponent } from '../../error-text/error-text.component';
import { RouterLink } from '@angular/router';
import { Roaster } from '../../../models/roaster';
import { HotToastService } from '@ngxpert/hot-toast';
import { SimpleModalComponent } from '../../modal/modal.component';

@Component({
  selector: 'app-roaster-manager2',
  imports: [
    TextInputComponent,
    PaginationControlsComponent,
    ReactiveFormsModule,
    ErrorTextComponent,
    RouterLink,
    SimpleModalComponent,
  ],
  templateUrl: './roaster-manager.component.html',
  styleUrls: [
    '../../roaster-directory/roaster-directory.component.scss',
    './roaster-manager.component.scss',
  ],
})
export class RoasterManagerComponent extends RoasterDirectoryComponent {
  private toast = inject(HotToastService);
  selectedRoaster: Roaster | null = null;
  deletedRoasterIds = new Set<number>();

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
        const deletedRoaster = this.paginatedResultSignal()?.items?.find(
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
}
