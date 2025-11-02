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
        this.editRoasterForm.patchValue(roaster);
      },
    });
  }

  initializeForm() {
    this.editRoasterForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      alias: ['', [Validators.maxLength(200)]],
      locationAddress: ['', [Validators.maxLength(500)]],
      websiteUrl: [
        '',
        [Validators.maxLength(300), Validators.pattern(VALID_URL_REGEX)],
      ],
      description: ['', [Validators.maxLength(2000)]],
    });
  }

  onEditRoaster() {
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
    const updateRoasterDto = {
      ...this.editRoasterForm.value,
      id: this.id,
    };
    this.roastersService.updateRoaster(this.id, updateRoasterDto).subscribe({
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
