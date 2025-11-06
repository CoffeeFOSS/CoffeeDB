import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

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

export function requireOtherControlValidator(
  otherControlName: string,
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.parent) return null;

    const otherControl = control.parent.get(otherControlName);
    if (!otherControl) return null;

    const thisValue = control.value;
    const otherValue = otherControl.value;

    if (otherControl.errors?.['requireOtherMismatch']) {
      const { requireOtherMismatch, ...rest } = otherControl.errors;
      otherControl.setErrors(Object.keys(rest).length ? rest : null);
    }

    // set error on the OTHER control if it’s missing and this has a value
    if (
      thisValue &&
      (otherValue === null || otherValue === undefined || otherValue === '')
    ) {
      otherControl.setErrors({
        ...otherControl.errors,
        requireOtherMismatch: true,
      });
    }

    return null;
  };
}
