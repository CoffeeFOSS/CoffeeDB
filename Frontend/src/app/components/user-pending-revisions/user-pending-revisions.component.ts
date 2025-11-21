import { Component, inject, OnInit } from '@angular/core';
import { LoadingService } from '../../services/loading.service';
import { RouterLink } from '@angular/router';
import { RevisionsService } from '../../services/revisions.service';
import { RevisionMetadataWithEntityIdentifier } from '../../models/revision';
import { getReadableDate } from '../../utils/date.utils';
import { getEntityBaseUrlFromEntityName } from '../../utils/entity.utils';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-user-pending-revisions',
  imports: [RouterLink],
  templateUrl: './user-pending-revisions.component.html',
  styleUrl: './user-pending-revisions.component.scss',
})
export class UserPendingRevisionsComponent implements OnInit {
  loadingService = inject(LoadingService);
  loadingKey = 'user-pending-revisions'; // dont need specific key because there can only be one logged in user
  private revisionsService = inject(RevisionsService);
  accountService = inject(AccountService);
  pendingRevisions: RevisionMetadataWithEntityIdentifier[] = [];

  ngOnInit(): void {
    const currentUser = this.accountService.currentUser();
    if (!currentUser) return;

    this.revisionsService.getUserPendingRevisions(currentUser.id).subscribe({
      next: (pendingRevisions: RevisionMetadataWithEntityIdentifier[]) => {
        this.pendingRevisions = pendingRevisions;
        this.loadingService.idle(this.loadingKey);
      },
      error: () => {
        this.loadingService.idle(this.loadingKey);
      },
    });
  }

  getEntityBaseUrl(entityName: string) {
    return getEntityBaseUrlFromEntityName(entityName);
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }
}
