import { Component, effect, inject } from '@angular/core';
import { LoadingService } from '../../services/loading.service';
import { RouterLink } from '@angular/router';
import { UserFrameService } from '../../services/user-frame.service';
import { RevisionsService } from '../../services/revisions.service';
import { RevisionMetadataContribution } from '../../models/revision';
import { getReadableDate } from '../../utils/date.utils';
import { getEntityBaseUrlFromEntityName } from '../../utils/entity.utils';

@Component({
  selector: 'app-user-contributions',
  imports: [RouterLink],
  templateUrl: './user-contributions.component.html',
  styleUrl: './user-contributions.component.scss',
})
export class UserContributionsComponent {
  loadingService = inject(LoadingService);
  loadingKey: string = '';
  private revisionsService = inject(RevisionsService);
  userFrameService = inject(UserFrameService);

  constructor() {
    this.loadingKey = `user-contributions-${this.userFrameService.user()?.username}`;

    effect(() => {
      const user = this.userFrameService.user();
      const contributions = this.userFrameService.userContributions();

      if (user && !contributions) {
        this.loadingService.busy(this.loadingKey);
        this.revisionsService.getUserRevisionContributions(user.id).subscribe({
          next: (userContributions: RevisionMetadataContribution[]) => {
            this.userFrameService.userContributions.set(userContributions);
            this.loadingService.idle(this.loadingKey);
          },
        });
      }
    });
  }

  getEntityBaseUrl(entityName: string) {
    return getEntityBaseUrlFromEntityName(entityName);
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }
}
