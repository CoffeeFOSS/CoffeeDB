import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { matchValues } from '../../utils/form.utils';
import { FormCtaButtonComponent } from '../forms/form-cta-button/form-cta-button.component';
import { ChangePasswordPayload } from '../../models/account';

@Component({
  selector: 'app-user-settings',
  imports: [ReactiveFormsModule, TextInputComponent, FormCtaButtonComponent],
  templateUrl: './user-settings.component.html',
  styleUrl: './user-settings.component.scss',
})
export class UserSettingsComponent implements OnInit {
  accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  changePasswordForm: FormGroup = new FormGroup({});
  isChangingPassword = false;
  validationErrors: string[] = [];

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.changePasswordForm = this.fb.group({
      currentPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(64),
        ],
      ],
      newPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(64),
        ],
      ],
      confirmNewPassword: [
        '',
        [Validators.required, matchValues('newPassword')],
      ],
    });

    // Update validity of confirm password control when password changes
    this.changePasswordForm.controls['newPassword'].valueChanges.subscribe({
      next: () =>
        this.changePasswordForm.controls[
          'confirmNewPassword'
        ].updateValueAndValidity(),
    });
  }

  onToggleChangePassword() {
    if (this.isChangingPassword) {
      this.isChangingPassword = false;
      this.changePasswordForm.reset();
    } else {
      this.isChangingPassword = true;
    }
  }

  onChangePasswordSubmit() {
    if (!this.changePasswordForm.valid) {
      this.changePasswordForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      console.log(12312);
      return;
    }
    this.validationErrors = [];
    this.accountService.changePassword(
      this.changePasswordForm.value as ChangePasswordPayload,
    );
    // .subscribe({
    //   next: () => {
    //     toast.success("Password updated successfully")
    //   },
    //   error: (error) => {
    //     this.validationErrors.push(error);
    //   },
    // });
  }
}
