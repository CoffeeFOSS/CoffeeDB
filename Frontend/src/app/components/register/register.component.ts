import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { ActivatedRoute, Router } from '@angular/router';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, TextInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  registerForm: FormGroup = new FormGroup({});
  showPassword = false;
  redirectUrl: string = '/';
  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.redirectUrl = this.route.snapshot.queryParams['redirectUrl'] || '/';
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      // Credentials
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(20),
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
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });

    // Update validity of confirm password control when password changes
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true }; // this is returned when controls dont match
    };
  }

  toggleShowPassword() {
    this.showPassword = !this.showPassword;
  }

  onCreateAccount() {
    if (!this.registerForm.valid) {
      this.registerForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => {
        this.router.navigateByUrl(this.redirectUrl);
      },
      error: (error) => {
        this.validationErrors = error;
      },
    });
  }
}
