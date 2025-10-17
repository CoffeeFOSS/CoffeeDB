import { AbstractControl, ValidatorFn } from '@angular/forms';

export function matchValues(matchTo: string): ValidatorFn {
  return (control: AbstractControl) => {
    return control.value === control.parent?.get(matchTo)?.value
      ? null
      : { matchValues: true }; // this is returned when controls dont match
  };
}

export function dontMatchValues(matchTo: string): ValidatorFn {
  return (control: AbstractControl) => {
    return control.value !== control.parent?.get(matchTo)?.value
      ? null
      : { dontMatchValues: true };
  };
}

export function dontMatchString(matchTo: string | undefined): ValidatorFn {
  return (control: AbstractControl) => {
    if (!matchTo || !control.value) return null;
    return control.value !== matchTo ? null : { dontMatchString: true };
  };
}
