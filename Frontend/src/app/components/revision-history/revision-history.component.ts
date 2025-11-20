import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RevisionMetadataExcerpt } from '../../models/revision';
import { RoastersService } from '../../services/roasters.service';
import { HotToastService } from '@ngxpert/hot-toast';
import { LoadingService } from '../../services/loading.service';
import { getReadableDate } from '../../utils/date.utils';
import { HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-revision-history',
  imports: [RouterLink],
  templateUrl: './revision-history.component.html',
  styleUrl: './revision-history.component.scss',
})
export class RevisionHistoryComponent implements OnInit {
  private roastersService = inject(RoastersService);
  private toast = inject(HotToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);
  revisionMetadataExcerpts: RevisionMetadataExcerpt[] = [];
  roasterId?: number;
  roasterName?: string;

  // TODO: showCommittedOnly flag, need to pass in via revisionParams or smth
  // otherwise moderators will see pending all the time

  constructor() {
    const roasterId = Number(this.route.snapshot.paramMap.get('id'));
    if (roasterId === undefined || isNaN(roasterId)) return;
    this.loadingService.busy(`roaster-revision-history-${roasterId}`);
    this.roasterId = roasterId;
  }

  ngOnInit() {
    const entityType = this.router.url.split('/')[1];

    switch (entityType) {
      case 'roasters':
        this.initializeRoasterRevisions();
        break;
      default:
        console.error(
          `Entity type ${entityType} has not been handled in revision history!`,
        );
    }
  }

  initializeRoasterRevisions() {
    if (!this.roasterId) return;

    this.roastersService
      .getRoasterRevisionMetadataExcerpts(this.roasterId)
      .subscribe({
        next: (response: HttpResponse<RevisionMetadataExcerpt[]>) => {
          this.revisionMetadataExcerpts = response.body || [];
          this.roasterName = response.headers.get('Roaster-Name') || '';
          this.loadingService.idle(
            `roaster-revision-history-${this.roasterId}`,
          );
        },
        error: (error: any) => {
          console.error(error);
          this.toast.error(error);
          this.loadingService.idle(
            `roaster-revision-history-${this.roasterId}`,
          );
        },
      });
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }
}
