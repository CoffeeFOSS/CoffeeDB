import { Component, output } from '@angular/core';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss'],
})
export class SimpleModalComponent {
  close = output<void>();

  closeModal(): void {
    this.close.emit();
  }
}
