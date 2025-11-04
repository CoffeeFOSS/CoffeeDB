import {
  AbstractControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';

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
  controlNames: string[],
): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    if (!(group instanceof FormGroup)) return null;

    const isEmpty = (val: any) =>
      val === null || val === undefined || val === '';

    const controls = controlNames
      .map((name) => group.get(name))
      .filter((ctrl): ctrl is AbstractControl => !!ctrl);

    // Determine if any control has a value
    const anyFilled = controls.some((ctrl) => !isEmpty(ctrl.value));

    // Clear previous errors
    for (const ctrl of controls) {
      if (ctrl.errors?.['requireOtherMismatch']) {
        const { requireOtherMismatch, ...rest } = ctrl.errors;
        ctrl.setErrors(Object.keys(rest).length ? rest : null);
      }
    }

    // If any field has a value, mark all empty fields as invalid
    if (anyFilled) {
      for (const ctrl of controls) {
        if (isEmpty(ctrl.value)) {
          ctrl.setErrors({
            ...ctrl.errors,
            requireOtherMismatch: true,
          });
        }
      }
    }

    return null; // group-level errors not needed
  };
}
