import { Component, effect, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserFrameService } from '../../services/user-frame.service';
import { RevisionsService } from '../../services/revisions.service';
import { RevisionMetadataWithEntityIdentifier } from '../../models/revision';
import { getReadableDate } from '../../utils/date.utils';
import { getEntityBaseUrlFromEntityName } from '../../utils/entity.utils';
import { PaginatedDirectoryComponent } from '../abstract/paginated-directory/paginated-directory.component';
import { FormKeyMap } from '../../utils/params.utils';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';

@Component({
  selector: 'app-user-contributions',
  imports: [RouterLink, PaginationControlsComponent],
  templateUrl: './user-contributions.component.html',
  styleUrl: './user-contributions.component.scss',
})
export class UserContributionsComponent extends PaginatedDirectoryComponent<
  RevisionMetadataWithEntityIdentifier,
  RevisionsService
> {
  protected service = inject(RevisionsService);
  protected items = signal<RevisionMetadataWithEntityIdentifier[]>([]);
  protected loadingKey = '';
  protected formKeyMap: FormKeyMap = {}; // intentionally empty

  userFrameService = inject(UserFrameService);

  constructor() {
    super();
    this.loadingKey = `user-contributions-${this.userFrameService.user()?.username}`;

    effect(() => {
      this.fetchItems();
    });
  }

  protected fetchPaginatedItems() {
    const user = this.userFrameService.user();
    if (!user) return;

    return this.service.getUserRevisionContributions(
      {
        page: this.paginationSignals.page.signal(),
        pageSize: this.paginationSignals.pageSize.signal(),
      },
      user.id,
    );
  }

  getEntityBaseUrl(entityName: string) {
    return getEntityBaseUrlFromEntityName(entityName);
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }
}
