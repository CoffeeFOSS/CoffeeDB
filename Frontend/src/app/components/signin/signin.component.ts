import { Component, inject, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
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
  redirectUrl: string = '/';
  validationErrors: string[] = [];

  ngOnInit(): void {
    this.redirectUrl = this.route.snapshot.queryParams['redirectUrl'] || '/';
    this.initializeForm();
  }

  initializeForm() {
    this.signInForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { matchValues: true }; // this is returned when controls dont match
    };
  }

  onSignIn() {
    if (!this.signInForm.valid) {
      this.signInForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.accountService.signIn(this.signInForm.value).subscribe({
      next: () => {
        this.validationErrors = [];
        this.router.navigateByUrl(this.redirectUrl);
      },
      error: (error) => {
        this.validationErrors = [error];
      },
    });
  }

  onNavigateRegister() {
    this.router.navigate(['/register'], {
      queryParams: { redirectUrl: this.redirectUrl },
    });
  }
}
