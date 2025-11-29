import { Component, inject } from '@angular/core';
import { RoasterEditComponent } from '../roaster-edit/roaster-edit.component';
import { ActivatedRoute } from '@angular/router';
import { RoasterRevisionSnapshot } from '../../models/roaster';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { TextAreaComponent } from '../forms/text-area/text-area.component';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-roaster-revision-edit',
  imports: [
    FormCtaButtonComponent,
    TextAreaComponent,
    TextInputComponent,
    ReactiveFormsModule,
  ],
  templateUrl: './roaster-revision-edit.component.html',
  styleUrls: [
    '../roaster-edit/roaster-edit.component.scss',
    './roaster-revision-edit.component.scss',
  ],
})
export class RoasterRevisionEditComponent extends RoasterEditComponent {
  private route = inject(ActivatedRoute);
  revisionId?: number;

  override loadRoaster() {
    this.loadRoasterRevision();
  }

  override updateRoaster() {
    this.updateRoasterRevision();
  }

  loadRoasterRevision() {
    // load roaster revision instead
    const revisionIdFromRoute = Number(
      this.route.snapshot.paramMap.get('revisionId'),
    );
    if (isNaN(revisionIdFromRoute)) return;

    this.roastersService
      .getRoasterRevisionSnapshot(revisionIdFromRoute)
      .subscribe({
        next: (roasterRevisionSnapshot: RoasterRevisionSnapshot) => {
          const {
            id,
            name,
            alias,
            locationAddress,
            locationCoordinates,
            websiteUrl,
            description,
            comment,
            roasterId,
          } = roasterRevisionSnapshot;

          this.revisionId = id;

          // original state
          this.roaster = {
            comment,
            id: roasterId ?? 0, // this doesn't matter for editing the revision, but we want typescript to be happy
            name,
            alias,
            locationAddress,
            locationCoordinates,
            websiteUrl,
            description,
          };
          this.editRoasterForm.patchValue({
            comment: comment ?? '',
            name: name ?? '',
            alias: alias ?? '',
            locationAddress: locationAddress ?? '',
            websiteUrl: websiteUrl ?? '',
            description: description ?? '',
            latitude: locationCoordinates?.latitude,
            longitude: locationCoordinates?.longitude,
          });
        },
      });
  }

  updateRoasterRevision() {
    if (!this.revisionId) return;

    const loadingId = `edit-roaster-revision-${this.revisionId}`;
    this.loadingService.busy(loadingId);
    this.roastersService
      .updateRoasterRevision(this.revisionId, this.getUpdatePayload())
      .subscribe({
        next: (roasterRevisionSnapshot: RoasterRevisionSnapshot) => {
          this.updateRoasterNext(roasterRevisionSnapshot, loadingId);
        },
        error: (error) => {
          this.updateRoasterError(error, loadingId);
        },
      });
  }
}
