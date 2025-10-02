import { Component, inject } from '@angular/core';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-test-errors',
  imports: [],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.scss',
})
export class TestErrorsComponent {
  toasts = inject(HotToastService);

  // https://ngxpert.github.io/hot-toast/
  onClickToastError() {
    this.toasts.error('This is an error message');
  }
  onClickToastSuccess() {
    this.toasts.success('This is a success message');
  }
  onClickToastInfo() {
    this.toasts.info('This is an info message');
  }
}
