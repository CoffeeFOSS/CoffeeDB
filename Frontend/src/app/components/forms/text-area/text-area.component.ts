import { Component, input, Self } from '@angular/core';
import {
  NgControl,
  FormControl,
  ReactiveFormsModule,
  ControlValueAccessor,
} from '@angular/forms';

@Component({
  selector: 'app-text-area',
  imports: [ReactiveFormsModule],
  templateUrl: './text-area.component.html',
  styleUrl: './text-area.component.scss',
})
export class TextAreaComponent implements ControlValueAccessor {
  label = input<string>('');
  type = input<string>('text');
  errorMessages = input<Record<string, string>>({});

  onChange = (_: any) => {};
  onTouched = () => {};

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
}
