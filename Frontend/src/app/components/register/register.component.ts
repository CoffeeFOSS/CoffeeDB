import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { ActivatedRoute, Router } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { matchValues } from '../../utils/form.utils';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, TextInputComponent, FormCtaButtonComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  registerForm: FormGroup = new FormGroup({});
  redirectUrl: string = '/';
  validationErrors: string[] = [];

  ngOnInit(): void {
    this.redirectUrl = this.route.snapshot.queryParams['redirectUrl'] || '/';
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(20),
          Validators.pattern(/^[a-zA-Z0-9-]+$/),
        ],
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(64),
        ],
      ],
      confirmPassword: ['', [Validators.required, matchValues('password')]],
    });

    // Update validity of confirm password control when password changes
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  onCreateAccount() {
    if (!this.registerForm.valid) {
      this.registerForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.validationErrors = [];
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => {
        this.router.navigateByUrl(this.redirectUrl);
      },
      error: (error) => {
        this.validationErrors.push(error);
      },
    });
  }
}
