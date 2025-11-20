import { Component, input } from '@angular/core';

@Component({
  selector: '[app-revision-diff-row]',
  imports: [],
  templateUrl: './revision-diff-row.component.html',
  styleUrl: './revision-diff-row.component.scss',
})
export class RevisionDiffRowComponent {
  key = input.required<string>();
  value = input<{ old: any; new: any }>();
  oldEmptyFallback = input<string>('');
  newEmptyFallback = input<string>('');
}
