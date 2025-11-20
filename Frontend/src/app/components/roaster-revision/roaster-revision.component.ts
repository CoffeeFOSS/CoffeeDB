import { Component, inject, OnInit } from '@angular/core';
import { LoadingService } from '../../services/loading.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RoastersService } from '../../services/roasters.service';
import {
  RoasterRevisionDiff,
  RoasterRevisionSnapshot,
} from '../../models/roaster';
import { RevisionDiffRowComponent } from '../revision-diff-row/revision-diff-row.component';

@Component({
  selector: 'app-roaster-revision',
  imports: [RevisionDiffRowComponent, RouterLink],
  templateUrl: './roaster-revision.component.html',
  styleUrl: './roaster-revision.component.scss',
})
export class RoasterRevisionComponent implements OnInit {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  roasterId: number | null = null;
  loadingService = inject(LoadingService);

  roasterRevisionDiff?: RoasterRevisionDiff;

  ngOnInit(): void {
    this.loadRoasterRevisionSnapshot();
  }

  loadRoasterRevisionSnapshot() {
    const { paramMap } = this.route.snapshot;
    const roasterId = Number(paramMap.get('roasterId'));

    const revisionId1 = Number(paramMap.get('revisionId1'));
    let revisionId2: number | null = Number(paramMap.get('revisionId2'));
    if (isNaN(revisionId1)) return;

    if (isNaN(revisionId2)) revisionId2 = null;
    if (revisionId2 == null && isNaN(roasterId)) return;

    this.roasterId = roasterId;

    if (revisionId2) {
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
    console.log(description);
    return description.split('\n').filter((p) => p.trim().length > 0);
  }

  getReadableDate(dateIsoString: string | null): string | null {
    if (!dateIsoString) return null;
    const date = new Date(dateIsoString);
    const hours12 = date.getHours() % 12 || 12;
    const ampm = date.getHours() >= 12 ? 'PM' : 'AM';
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')} at ${hours12}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')} ${ampm}`;
  }
}
