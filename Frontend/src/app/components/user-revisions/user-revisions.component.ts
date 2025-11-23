import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RevisionsService } from '../../services/revisions.service';
import {
  RevisionMetadataWithEntityIdentifier,
  RevisionStatusEnum,
} from '../../models/revision';
import { getReadableDate } from '../../utils/date.utils';
import { getEntityBaseUrlFromEntityName } from '../../utils/entity.utils';
import { AccountService } from '../../services/account.service';
import { PaginatedDirectoryComponent } from '../abstract/paginated-directory/paginated-directory.component';
import { FormKeyMap } from '../../utils/params.utils';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { LabeledCheckboxComponent } from '../labeled-checkbox/labeled-checkbox.component';
import { UserFrameService } from '../../services/user-frame.service';

@Component({
  selector: 'app-user-revisions',
  imports: [RouterLink, PaginationControlsComponent, LabeledCheckboxComponent],
  templateUrl: './user-revisions.component.html',
  styleUrl: './user-revisions.component.scss',
})
export class UserRevisionsComponent extends PaginatedDirectoryComponent<
  RevisionMetadataWithEntityIdentifier,
  RevisionsService
> {
  protected service = inject(RevisionsService);
  protected items = signal<RevisionMetadataWithEntityIdentifier[]>([]);
  protected loadingKey = 'user-revisions'; // dont need specific key because there can only be one logged in user
  protected formKeyMap: FormKeyMap = {}; // intentionally empty
  userFrameService = inject(UserFrameService);

  revisionStatuses = Object.entries(RevisionStatusEnum)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({
      value: value as RevisionStatusEnum,
      label: key,
    }));

  selectedStatuses = signal(new Set<number>());

  accountService = inject(AccountService);

  constructor() {
    super();
  }

  protected fetchPaginatedItems() {
    const currentUser = this.accountService.currentUser();
    if (!currentUser) return;

    return this.service.getUserRevisions(
      {
        status:
          this.selectedStatuses().size > 0
            ? Array.from(this.selectedStatuses()).join(',')
            : undefined,
        page: this.paginationSignals.page.signal(),
        pageSize: this.paginationSignals.pageSize.signal(),
      },
      currentUser.id,
    );
  }

  getEntityBaseUrl(entityName: string) {
    return getEntityBaseUrlFromEntityName(entityName);
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }

  onToggleStatus(statusValue: string) {
    const numStatusValue = Number(statusValue);
    if (isNaN(numStatusValue)) return;

    this.selectedStatuses.update((set) => {
      if (set.has(numStatusValue)) {
        set.delete(numStatusValue);
      } else {
        set.add(numStatusValue);
      }
      return set;
    });
  }

  async requery() {
    this.fetchItems({ bypassQueryCache: true });
  }
}
