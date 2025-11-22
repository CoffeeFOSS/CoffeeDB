import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RevisionsService } from '../../services/revisions.service';
import { RevisionMetadataWithEntityIdentifier } from '../../models/revision';
import { getReadableDate } from '../../utils/date.utils';
import { getEntityBaseUrlFromEntityName } from '../../utils/entity.utils';
import { AccountService } from '../../services/account.service';
import { PaginatedDirectoryComponent } from '../abstract/paginated-directory/paginated-directory.component';
import { FormKeyMap } from '../../utils/params.utils';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';

@Component({
  selector: 'app-user-pending-revisions',
  imports: [RouterLink, PaginationControlsComponent],
  templateUrl: './user-pending-revisions.component.html',
  styleUrl: './user-pending-revisions.component.scss',
})
export class UserPendingRevisionsComponent extends PaginatedDirectoryComponent<
  RevisionMetadataWithEntityIdentifier,
  RevisionsService
> {
  protected service = inject(RevisionsService);
  protected items = signal<RevisionMetadataWithEntityIdentifier[]>([]);
  protected loadingKey = 'user-pending-revisions'; // dont need specific key because there can only be one logged in user
  protected formKeyMap: FormKeyMap = {}; // intentionally empty

  accountService = inject(AccountService);

  constructor() {
    super();
  }

  protected fetchPaginatedItems() {
    const currentUser = this.accountService.currentUser();
    if (!currentUser) return;

    return this.service.getUserPendingRevisions(
      {
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
}
