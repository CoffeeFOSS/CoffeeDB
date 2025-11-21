import { Component, inject, OnInit } from '@angular/core';
import { LoadingService } from '../../services/loading.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { RoastersService } from '../../services/roasters.service';
import {
  LocationCoordinates,
  RoasterRevisionDiff,
  RoasterRevisionSnapshot,
} from '../../models/roaster';
import { RevisionDiffRowComponent } from '../revision-diff-row/revision-diff-row.component';
import {
  getCoordinatesDiffParts,
  getMultilineDiffParts,
} from '../../utils/diff.utils';
import { getReadableDate } from '../../utils/date.utils';
import { RoastersFrameService } from '../../services/roaster-frame.service';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-roaster-revision',
  imports: [RevisionDiffRowComponent, RouterLink],
  templateUrl: './roaster-revision.component.html',
  styleUrls: [
    '../revision-diff-row/revision-diff-row.component.scss',
    './roaster-revision.component.scss',
  ],
})
export class RoasterRevisionComponent implements OnInit {
  private roastersService = inject(RoastersService);
  accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  roasterId: number | null = null;
  loadingService = inject(LoadingService);
  loadingKey = '';
  roastersFrameService = inject(RoastersFrameService);

  roasterRevisionDiff?: RoasterRevisionDiff;

  ngOnInit(): void {
    // Not using roastersFrameService to load cache in here, since I dont expect users to go back and forth between revisions
    // and even if they do, the revisions would get replaced anyways, so caching wouldn't do anything.
    // If we want to implement caching in the future, we should do a queryKey mapped caching
    this.loadRoasterRevisionSnapshot();
  }

  loadRoasterRevisionSnapshot() {
    const { paramMap } = this.route.snapshot;
    const revisionId1 = Number(paramMap.get('revisionId1'));
    let revisionId2: number | null = Number(paramMap.get('revisionId2'));
    let roasterId = this.roastersFrameService.roasterId();

    this.loadingKey = `revision-${roasterId}-${revisionId1}-${revisionId2}`;
    this.loadingService.busy(this.loadingKey);
    if (revisionId2 && roasterId) {
      this.roastersService
        .getRoasterRevisionDiffs(roasterId, revisionId1, revisionId2)
        .subscribe({
          next: (roasterRevisionDiff: RoasterRevisionDiff) => {
            this.roasterRevisionDiff = roasterRevisionDiff;
            if (this.roasterRevisionDiff.changes.createdAt) {
              this.roasterRevisionDiff.changes.createdAt.old =
                this.getReadableDate(
                  this.roasterRevisionDiff.changes.createdAt.old,
                );
              this.roasterRevisionDiff.changes.createdAt.new =
                this.getReadableDate(
                  this.roasterRevisionDiff.changes.createdAt.new,
                );
            }
            this.loadingService.idle(this.loadingKey);
          },
          error: () => {
            this.loadingService.idle(this.loadingKey);
          },
        });
    } else {
      this.roastersService.getRoasterRevisionSnapshot(revisionId1).subscribe({
        next: (roasterRevisionSnapshot: RoasterRevisionSnapshot) => {
          if (roasterRevisionSnapshot.parentRevisionId != null) return;

          const {
            roasterId: rId,
            id,
            createdAt,
            createdBy,
            version,
            comment,
            name,
            alias,
            locationAddress,
            locationCoordinates,
            websiteUrl,
            description,
          } = roasterRevisionSnapshot;
          this.roasterRevisionDiff = {
            roasterId: rId,
            changes: {
              id: {
                old: null,
                new: id,
              },
              createdAt: {
                old: null,
                new: createdAt ? this.getReadableDate(createdAt) : null,
              },
              createdBy: {
                old: null,
                new: createdBy ?? null,
              },
              version: {
                old: null,
                new: version ?? null,
              },
              comment: {
                old: null,
                new: comment,
              },
              // properties that are only added in diff if they are changed
              name:
                name == null
                  ? undefined
                  : {
                      old: null,
                      new: name,
                    },
              alias:
                alias == null
                  ? undefined
                  : {
                      old: null,
                      new: alias,
                    },
              locationAddress:
                locationAddress == null
                  ? undefined
                  : {
                      old: null,
                      new: locationAddress,
                    },
              locationCoordinates:
                locationCoordinates == null
                  ? undefined
                  : {
                      old: null,
                      new: locationCoordinates,
                    },
              websiteUrl:
                websiteUrl == null
                  ? undefined
                  : {
                      old: null,
                      new: websiteUrl,
                    },
              description:
                description == null
                  ? undefined
                  : {
                      old: null,
                      new: description,
                    },
            },
          };
          this.loadingService.idle(this.loadingKey);
        },
        error: () => {
          this.loadingService.idle(this.loadingKey);
        },
      });
    }
  }

  getEntries() {
    if (!this.roasterRevisionDiff) return [];
    return Object.entries(this.roasterRevisionDiff.changes);
  }

  getParagraphs(description: string): string[] {
    if (!description) {
      return [];
    }
    return description.split('\n').filter((p) => p.trim().length > 0);
  }

  getReadableDate(dateIsoString: string | null): string | null {
    return getReadableDate(dateIsoString);
  }

  public getMultilineDiffParts(
    isNewDiff: boolean,
    oldVal?: string | null,
    newVal?: string | null,
    fallback: string = '',
  ) {
    return getMultilineDiffParts(isNewDiff, oldVal, newVal, fallback);
  }

  public getCoordinatesDiffParts(
    isNewDiff: boolean,
    oldCoords?: LocationCoordinates | null,
    newCoords?: LocationCoordinates | null,
    fallback: string = '',
  ) {
    return getCoordinatesDiffParts(isNewDiff, oldCoords, newCoords, fallback);
  }
}
