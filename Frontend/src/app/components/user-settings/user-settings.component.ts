import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  dontMatchString,
  dontMatchValues,
  matchValues,
} from '../../utils/form.utils';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { HotToastService } from '@ngxpert/hot-toast';
import { LoadingService } from '../../services/loading.service';

@Component({
  selector: 'app-user-settings',
  imports: [ReactiveFormsModule, TextInputComponent, FormCtaButtonComponent],
  templateUrl: './user-settings.component.html',
  styleUrl: './user-settings.component.scss',
})
export class UserSettingsComponent implements OnInit {
  accountService = inject(AccountService);
  loadingService = inject(LoadingService);
  private fb = inject(FormBuilder);
  changeUsernameForm: FormGroup = new FormGroup({});
  changePasswordForm: FormGroup = new FormGroup({});
  isChangingUsername = false;
  isChangingPassword = false;
  passwordValidationErrors: string[] = [];
  usernameValidationErrors: string[] = [];
  private toast = inject(HotToastService);

  ngOnInit(): void {
    this.initializeForms();
  }

  initializeForms() {
    this.changeUsernameForm = this.fb.group({
      newUsername: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(20),
          Validators.pattern(/^[a-zA-Z0-9-]+$/),
          dontMatchString(this.accountService.currentUser()?.username),
        ],
      ],
      password: ['', [Validators.required]],
    });

    this.changePasswordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(64),
          dontMatchValues('currentPassword'),
        ],
      ],
      confirmNewPassword: [
        '',
        [Validators.required, matchValues('newPassword')],
      ],
    });

    this.changePasswordForm.controls['newPassword'].valueChanges.subscribe({
      next: () =>
        this.changePasswordForm.controls[
          'confirmNewPassword'
        ].updateValueAndValidity(),
    });
  }

  onToggleChangeUsername() {
    if (this.isChangingUsername) {
      this.isChangingUsername = false;
      this.changeUsernameForm.reset();
    } else {
      this.isChangingUsername = true;
    }
  }

  onToggleChangePassword() {
    if (this.isChangingPassword) {
      this.isChangingPassword = false;
      this.changePasswordForm.reset();
    } else {
      this.isChangingPassword = true;
    }
  }

  onChangeUsernameSubmit() {
    if (!this.changeUsernameForm.valid) {
      this.changeUsernameForm.markAllAsTouched();
      this.usernameValidationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.loadingService.busy('change-username');
    this.accountService
      .changeUsername(this.changeUsernameForm.value)
      .subscribe({
        next: () => {
          this.toast.success('Username updated successfully');
          this.usernameValidationErrors = [];
          this.loadingService.idle('change-username');
          this.onToggleChangeUsername();
        },
        error: (error) => {
          this.loadingService.idle('change-username');
          this.usernameValidationErrors = [error];
        },
      });
  }

  onChangePasswordSubmit() {
    if (!this.changePasswordForm.valid) {
      this.changePasswordForm.markAllAsTouched();
      this.passwordValidationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.loadingService.busy('change-password');
    this.accountService
      .changePassword(this.changePasswordForm.value)
      .subscribe({
        next: () => {
          this.toast.success('Password updated successfully');
          this.passwordValidationErrors = [];
          this.loadingService.idle('change-password');
          this.onToggleChangePassword();
        },
        error: (error) => {
          this.loadingService.idle('change-password');
          this.passwordValidationErrors = [error];
        },
      });
  }
}
