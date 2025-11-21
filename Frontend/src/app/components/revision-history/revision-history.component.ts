import { Component, effect, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RevisionMetadataExcerpt } from '../../models/revision';
import { RoastersService } from '../../services/roasters.service';
import { HotToastService } from '@ngxpert/hot-toast';
import { LoadingService } from '../../services/loading.service';
import { getReadableDate } from '../../utils/date.utils';
import { HttpResponse } from '@angular/common/http';
import { getEntityBaseUrlFromEntityPath } from '../../utils/entity.utils';
import { RoastersFrameService } from '../../services/roaster-frame.service';

@Component({
  selector: 'app-revision-history',
  imports: [RouterLink],
  templateUrl: './revision-history.component.html',
  styleUrl: './revision-history.component.scss',
})
export class RevisionHistoryComponent {
  private roastersService = inject(RoastersService);
  roastersFrameService = inject(RoastersFrameService);
  private toast = inject(HotToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);
  loadingKey: string = '';
  entityId?: number;
  entityPath?: string;
  entityName?: string;

  constructor() {
    // This component can be part of any Roaster/Grinder/etc frame, so get generic id instead of relying on Entity Frames
    const id = Number(this.route.parent?.snapshot.paramMap.get('id'));
    if (id === undefined || isNaN(id)) return;

    const parentUrlSegments = this.route.parent?.snapshot.url;
    const firstSegment =
      parentUrlSegments && parentUrlSegments.length > 0
        ? parentUrlSegments[0].path
        : null;
    this.entityPath = firstSegment || undefined;
    this.loadingKey = `${this.entityPath}-revision-history-${id}`;
    this.entityId = id;

    effect(() => {
      let revisions: RevisionMetadataExcerpt[] | null = null;
      switch (this.entityPath) {
        case 'roasters':
          revisions = this.roastersFrameService.roasterRevisionHistory();
          break;
        default:
      }

      if (!revisions) {
        const entityType = this.router.url.split('/')[1];
        this.initializeRevisions(entityType);
      }
    });
  }

  initializeRevisions(entityType: string) {
    if (!this.entityId) return;
    this.loadingService.busy(this.loadingKey);

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
    if (!this.entityId) return;

    this.loadingService.busy(this.loadingKey);
    this.roastersService
      .getRoasterRevisionMetadataExcerpts(this.entityId, true)
      .subscribe({
        next: (response: HttpResponse<RevisionMetadataExcerpt[]>) => {
          this.roastersFrameService.roasterRevisionHistory.set(
            response.body || [],
          );
          this.entityName = response.headers.get('Roaster-Name') || '';
          this.loadingService.idle(this.loadingKey);
        },
        error: (error: any) => {
          console.error(error);
          this.toast.error(error);
          this.loadingService.idle(this.loadingKey);
        },
      });
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }

  getEntityBaseUrl(entityName: string) {
    return getEntityBaseUrlFromEntityPath(entityName);
  }

  getRevisionMetadata() {
    switch (this.entityPath) {
      case 'roasters':
        return this.roastersFrameService.roasterRevisionHistory();
      default:
        return [];
    }
  }
}
