import { Component, inject, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../../services/account.service';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { TextInputComponent } from '../forms/text-input/text-input.component';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [ReactiveFormsModule, FormCtaButtonComponent, TextInputComponent],
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.scss',
})
export class SignInComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  signInForm: FormGroup = new FormGroup({});
  showPassword = false;
  redirectUrl: string = '/';
  validationErrors: string[] = [];

  ngOnInit(): void {
    this.redirectUrl = this.route.snapshot.queryParams['redirectUrl'] || '/';
    this.initializeForm();
  }

  initializeForm() {
    this.signInForm = this.fb.group({
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
    });

    // Update validity of confirm password control when password changes
    this.signInForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.signInForm.controls['confirmPassword'].updateValueAndValidity(),
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
    this.showPassword = !this.showPassword; // TODO: Delete this
  }

  onSignIn() {
    if (!this.signInForm.valid) {
      this.signInForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.validationErrors = [];
    this.accountService.signIn(this.signInForm.value).subscribe({
      next: () => {
        this.router.navigateByUrl(this.redirectUrl);
      },
      error: (error) => {
        console.error(error);
        this.validationErrors.push(error);
      },
    });
  }

  onNavigateRegister() {
    this.router.navigate(['/register'], {
      queryParams: { redirectUrl: this.redirectUrl },
    });
  }
}
