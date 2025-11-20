import { Component, input, ViewEncapsulation } from '@angular/core';
import { DiffPart, getDiffParts } from '../../utils/diff.utils';

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
  runDiff = input<boolean>(false);

  get oldDiffParts(): DiffPart[] {
    if (!this.runDiff())
      return [{ value: this.value()?.old ?? this.oldEmptyFallback() }];
    return getDiffParts(
      false,
      this.value()?.old,
      this.value()?.new,
      this.oldEmptyFallback(),
    );
  }

  get newDiffParts(): DiffPart[] {
    if (!this.runDiff())
      return [{ value: this.value()?.new ?? this.newEmptyFallback() }];
    return getDiffParts(
      true,
      this.value()?.old,
      this.value()?.new,
      this.newEmptyFallback(),
    );
  }
}
