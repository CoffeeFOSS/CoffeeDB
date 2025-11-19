import { Component, inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Roaster } from '../../models/roaster';
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
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  id: number | null = null;
  loadingService = inject(LoadingService);
  roaster?: Roaster;

  private fb = new FormBuilder();

  editRoasterForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  submitted = false;
  coordinateGroup = ['latitude', 'longitude'];

  ngOnInit(): void {
    this.initializeForm();
    this.loadRoaster();
  }

  loadRoaster() {
    const roasterId = Number(this.route.snapshot.paramMap.get('id'));
    if (roasterId === undefined || isNaN(roasterId)) return;
    this.id = roasterId;

    this.roastersService.getRoaster(roasterId).subscribe({
      next: (roaster: Roaster) => {
        this.roaster = roaster;
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
          name,
          alias,
          locationAddress,
          websiteUrl,
          description,
          latitude: locationCoordinates?.latitude,
          longitude: locationCoordinates?.longitude,
        });
      },
    });
  }

  initializeForm() {
    this.editRoasterForm = this.fb.group({
      comment: ['', [Validators.required, Validators.maxLength(300)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      alias: ['', [Validators.maxLength(200)]],
      locationAddress: ['', [Validators.maxLength(500)]],
      latitude: [
        null,
        [
          Validators.min(-90),
          Validators.max(90),
          requireAllControlsValidator(this.coordinateGroup),
        ],
      ],
      longitude: [
        null,
        [
          Validators.min(-180),
          Validators.max(180),
          requireAllControlsValidator(this.coordinateGroup),
        ],
      ],
      websiteUrl: [
        '',
        [Validators.maxLength(300), Validators.pattern(VALID_URL_REGEX)],
      ],
      description: ['', [Validators.maxLength(2000)]],
    });
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
    if (this.editRoasterForm.pristine) {
      this.validationErrors = ['No changes were made to the roaster details.'];
      return;
    }
    if (!this.id) {
      this.validationErrors = ['Roaster ID not found, is the URL correct?'];
      return;
    }
    const loadingId = `edit-roaster-${this.id}`;
    this.loadingService.busy(loadingId);
    this.roastersService
      .updateRoaster(this.id, {
        comment: this.editRoasterForm.value.comment,
        name: this.editRoasterForm.value.name,
        alias: this.editRoasterForm.value.alias ?? undefined,
        locationAddress:
          this.editRoasterForm.value.locationAddress ?? undefined,
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
      })
      .subscribe({
        next: (roaster: Roaster) => {
          this.validationErrors = [];
          this.loadingService.idle(loadingId);
          this.router.navigate(['/roasters', roaster.id]);
        },
        error: (error) => {
          this.loadingService.idle(loadingId);
          this.validationErrors = [error];
        },
      });
  }

  get isDataUnchanged(): boolean {
    if (!this.roaster) return true;
    const { id, ...originalData } = this.roaster;

    return objectsAreIdentical(this.editRoasterForm.value, originalData);
  }
}
