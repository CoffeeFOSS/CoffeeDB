import { Component, inject } from '@angular/core';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-test-errors',
  imports: [],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.scss',
})
export class TestErrorsComponent {
  toast = inject(HotToastService);

  // https://ngxpert.github.io/hot-toast/
  onClickToastError() {
    this.toast.error('This is an error message');
  }
  onClickToastSuccess() {
    this.toast.success('This is a success message');
  }
  onClickToastInfo() {
    this.toast.info('This is an info message');
  }
}
