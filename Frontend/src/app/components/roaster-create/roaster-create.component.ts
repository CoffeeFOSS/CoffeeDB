import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Roaster } from '../../models/roaster';
import { RoastersService } from '../../services/roasters.service';
import { LoadingService } from '../../services/loading.service';
import { Router } from '@angular/router';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { TextAreaComponent } from '../forms/text-area/text-area.component';

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
  loadingService = inject(LoadingService);
  private router = inject(Router);

  createRoasterForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.createRoasterForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      alias: ['', [Validators.maxLength(200)]],
      location: ['', [Validators.maxLength(500)]],
      websiteUrl: ['', [Validators.maxLength(300)]],
      description: ['', [Validators.maxLength(2000)]],
    });
  }

  onCreateRoaster() {
    if (!this.createRoasterForm.valid) {
      this.createRoasterForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.loadingService.busy('create-roaster');
    this.roastersService.createRoaster(this.createRoasterForm.value).subscribe({
      next: (roaster: Roaster) => {
        this.validationErrors = [];
        this.loadingService.idle('create-roaster');
        this.router.navigate(['/roasters', roaster.id]);
      },
      error: (error) => {
        this.loadingService.idle('create-roaster');
        this.validationErrors = [error];
      },
    });
  }
}
