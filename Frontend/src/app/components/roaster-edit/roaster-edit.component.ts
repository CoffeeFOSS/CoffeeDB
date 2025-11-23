import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  Roaster,
  RoasterOriginalData,
  RoasterRevisionSnapshot,
} from '../../models/roaster';
import { LoadingService } from '../../services/loading.service';
import { RoastersService } from '../../services/roasters.service';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { objectsAreIdentical } from '../../utils/objects.utils';
import { TextAreaComponent } from '../forms/text-area/text-area.component';
import { VALID_URL_REGEX } from '../../constants/regex.constants';
import { requireAllControlsValidator } from '../../utils/form.utils';
import { HotToastService } from '@ngxpert/hot-toast';
import { RoastersFrameService } from '../../services/roaster-frame.service';

@Component({
  selector: 'app-roaster-edit',
  imports: [
    ReactiveFormsModule,
    TextInputComponent,
    FormCtaButtonComponent,
    TextAreaComponent,
  ],
  templateUrl: './roaster-edit.component.html',
  styleUrl: './roaster-edit.component.scss',
})
export class RoasterEditComponent implements OnInit {
  protected roastersService = inject(RoastersService);
  roastersFrameService = inject(RoastersFrameService);
  private router = inject(Router);
  private toast = inject(HotToastService);
  id: number | null = null;
  loadingService = inject(LoadingService);
  roaster?: RoasterOriginalData;

  private fb = new FormBuilder();

  editRoasterForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  submitted = false;
  coordinateGroup = ['latitude', 'longitude'];

  ngOnInit(): void {
    this.initializeForm();
    // Wont be using roasterFrameService for caching in here, we'd best show the most up to date defaults to the user
    this.loadRoaster();
  }

  loadRoaster() {
    let roasterId = this.roastersFrameService.roasterId();
    if (!roasterId) return;

    this.roastersService
      .getRoaster(Number(this.roastersFrameService.roasterId()))
      .subscribe({
        next: (roaster: Roaster) => {
          this.roaster = {
            ...roaster,
            comment: '',
          };
          const {
            name,
            alias,
            locationAddress,
            locationCoordinates,
            websiteUrl,
            description,
          } = roaster;
          this.editRoasterForm.patchValue({
            comment: '',
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

  initializeForm() {
    this.editRoasterForm = this.fb.group(
      {
        comment: ['', [Validators.required, Validators.maxLength(300)]],
        name: ['', [Validators.required, Validators.maxLength(100)]],
        alias: ['', [Validators.maxLength(200)]],
        locationAddress: ['', [Validators.maxLength(500)]],
        latitude: ['', [Validators.min(-90), Validators.max(90)]],
        longitude: ['', [Validators.min(-180), Validators.max(180)]],
        websiteUrl: [
          '',
          [Validators.maxLength(300), Validators.pattern(VALID_URL_REGEX)],
        ],
        description: ['', [Validators.maxLength(2000)]],
      },
      {
        validators: requireAllControlsValidator(this.coordinateGroup),
      },
    );
  }

  onEditRoaster() {
    this.submitted = true;

    if (!this.editRoasterForm.valid) {
      this.editRoasterForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    if (this.editRoasterForm.pristine || this.getIsDataUnchanged()) {
      this.validationErrors = ['No changes were made to the roaster details.'];
      return;
    }
    this.updateRoaster();
  }

  updateRoaster() {
    const roasterId = this.roastersFrameService.roasterId();
    if (roasterId == null || roasterId == undefined) {
      this.validationErrors = ['Roaster ID not found, is the URL correct?'];
      return;
    }
    const loadingId = `edit-roaster-${roasterId}`;
    this.loadingService.busy(loadingId);
    this.roastersService
      .updateRoaster(roasterId, this.getUpdatePayload())
      .subscribe({
        next: (roasterRevisionSnapshot: RoasterRevisionSnapshot) => {
          this.updateRoasterNext(roasterRevisionSnapshot, loadingId);
        },
        error: (error) => {
          this.updateRoasterError(error, loadingId);
        },
      });
  }

  getUpdatePayload() {
    return {
      comment: this.editRoasterForm.value.comment,
      name: this.editRoasterForm.value.name,
      alias: this.editRoasterForm.value.alias ?? undefined,
      locationAddress: this.editRoasterForm.value.locationAddress ?? undefined,
      locationCoordinateLatitude:
        this.editRoasterForm.value.latitude != null &&
        this.editRoasterForm.value.latitude !== ''
          ? Number(this.editRoasterForm.value.latitude)
          : undefined,
      locationCoordinateLongitude:
        this.editRoasterForm.value.longitude != null &&
        this.editRoasterForm.value.longitude !== ''
          ? Number(this.editRoasterForm.value.longitude)
          : undefined,
      websiteUrl: this.editRoasterForm.value.websiteUrl ?? undefined,
      description: this.editRoasterForm.value.description ?? undefined,
    };
  }

  updateRoasterNext = (
    roasterRevisionSnapshot: RoasterRevisionSnapshot,
    loadingId: string,
  ) => {
    this.validationErrors = [];
    this.loadingService.idle(loadingId);
    this.router.navigate([
      '/roasters',
      this.roaster?.id ?? 'new',
      'revisions',
      roasterRevisionSnapshot.id,
      roasterRevisionSnapshot.parentRevisionId,
    ]);
    this.toast.success(
      `Roaster revision ID ${roasterRevisionSnapshot.id} successfully created! It will be reviewed by the moderation team shortly for Roaster Update.`,
    );
  };

  updateRoasterError = (error: any, loadingId: string) => {
    this.loadingService.idle(loadingId);
    this.validationErrors = [error];
  };

  getIsDataUnchanged(): boolean {
    if (!this.roaster) return true;
    const { id, distanceInKilometers, locationCoordinates, ...originalData } =
      this.roaster;

    const normalizedFormValue = {
      ...this.editRoasterForm.value,
      latitude: String(this.editRoasterForm.value.latitude),
      longitude: String(this.editRoasterForm.value.longitude),
    };

    const normalizedOriginalData = {
      ...originalData,
      latitude: String(locationCoordinates?.latitude),
      longitude: String(locationCoordinates?.longitude),
      description: originalData.description ?? '',
    };

    return objectsAreIdentical(normalizedFormValue, normalizedOriginalData);
  }
}
