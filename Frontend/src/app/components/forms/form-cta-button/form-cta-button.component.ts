import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-form-cta-button',
  imports: [],
  templateUrl: './form-cta-button.component.html',
  styleUrl: './form-cta-button.component.scss',
})
export class FormCtaButtonComponent {
  buttonClick = output<void>();
  text = input.required<string>();
  type = input<'button' | 'submit' | 'reset'>('button');
  variant = input.required<'ephemeral' | 'primary'>();
  disabled = input<boolean>(false);
}
