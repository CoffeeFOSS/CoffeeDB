import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RoasterRevisionSnapshot } from '../../models/roaster';
import { RoastersService } from '../../services/roasters.service';
import { LoadingService } from '../../services/loading.service';
import { Router } from '@angular/router';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { TextAreaComponent } from '../forms/text-area/text-area.component';
import { VALID_URL_REGEX } from '../../constants/regex.constants';
import { requireAllControlsValidator } from '../../utils/form.utils';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-roaster-create',
  imports: [
    ReactiveFormsModule,
    TextInputComponent,
    FormCtaButtonComponent,
    TextAreaComponent,
  ],
  templateUrl: './roaster-create.component.html',
  styleUrl: './roaster-create.component.scss',
})
export class RoasterCreateComponent implements OnInit {
  private roastersService = inject(RoastersService);
  private fb = new FormBuilder();
  private toast = inject(HotToastService);
  loadingService = inject(LoadingService);
  private router = inject(Router);

  createRoasterForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  submitted = false;
  coordinateGroup = ['latitude', 'longitude'];

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.createRoasterForm = this.fb.group({
      comment: [
        'Initial revision',
        [Validators.required, Validators.maxLength(300)],
      ],
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

  onCreateRoaster() {
    this.submitted = true;

    if (!this.createRoasterForm.valid) {
      this.createRoasterForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.loadingService.busy('create-roaster');
    this.roastersService
      .createRoaster({
        comment: this.createRoasterForm.value.comment,
        name: this.createRoasterForm.value.name,
        alias: this.createRoasterForm.value.alias ?? undefined,
        locationAddress:
          this.createRoasterForm.value.locationAddress ?? undefined,
        locationCoordinateLatitude:
          this.createRoasterForm.value.latitude != null &&
          this.createRoasterForm.value.latitude !== ''
            ? Number(this.createRoasterForm.value.latitude)
            : undefined,
        locationCoordinateLongitude:
          this.createRoasterForm.value.longitude != null &&
          this.createRoasterForm.value.longitude !== ''
            ? Number(this.createRoasterForm.value.longitude)
            : undefined,
        websiteUrl: this.createRoasterForm.value.websiteUrl ?? undefined,
        description: this.createRoasterForm.value.description ?? undefined,
      })
      .subscribe({
        next: (roasterRevisionSnapshot: RoasterRevisionSnapshot) => {
          this.validationErrors = [];
          this.loadingService.idle('create-roaster');
          this.router.navigateByUrl('/roasters');
          this.router.navigate([
            '/roasters/new/revisions',
            roasterRevisionSnapshot.id,
          ]);
          this.toast.success(
            `Roaster revision ID ${roasterRevisionSnapshot.id} successfully created! It will be reviewed by the moderation team shortly for Roaster Creation.`,
          );
        },
        error: (error) => {
          this.loadingService.idle('create-roaster');
          this.validationErrors = [error];
        },
      });
  }

  get isDataEmpty(): boolean {
    if (!this.createRoasterForm) return true;
    const formValue = this.createRoasterForm.value;

    for (const key in formValue) {
      if (formValue.hasOwnProperty(key)) {
        const value = formValue[key];
        if (typeof value === 'string' && value.trim().length > 0) return false;
        if (
          value !== null &&
          value !== undefined &&
          typeof value !== 'string'
        ) {
          return false;
        }
      }
    }

    return true;
  }
}
