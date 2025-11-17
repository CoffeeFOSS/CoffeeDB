import { Component, input, Self } from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NgControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { faEyeSlash, faEye } from '@ng-icons/font-awesome/regular';

@Component({
  selector: 'app-text-input',
  standalone: true,
  imports: [ReactiveFormsModule, NgIcon],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.scss',
})
export class TextInputComponent implements ControlValueAccessor {
  label = input<string>('');
  type = input<string>('text');
  autocomplete = input<string>();
  errorMessages = input<Record<string, string>>({});
  passwordVisible = false;
  showErrors = input<boolean>(true);
  showLabel = input<boolean>(true);

  hidePasswordIcon = faEye;
  showPasswordIcon = faEyeSlash;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {}

  registerOnChange(fn: any): void {}

  registerOnTouched(fn: any): void {}

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }

  get errorKeys(): string[] {
    return Object.keys(this.control?.errors || {});
  }

  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }
}
