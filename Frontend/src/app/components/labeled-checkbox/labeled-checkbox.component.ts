import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-labeled-checkbox',
  imports: [],
  templateUrl: './labeled-checkbox.component.html',
  styleUrl: './labeled-checkbox.component.scss',
})
export class LabeledCheckboxComponent {
  checked = input.required<boolean>();
  label = input.required<string>();
  disabled = input<boolean>(false);
  value = input.required<string>();
  toggle = output<string>();

  onClick() {
    if (!this.disabled()) {
      this.toggle.emit(this.value());
    }
  }
}
