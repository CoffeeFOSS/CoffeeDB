import { Injectable, signal } from '@angular/core';
import { Member } from '../models/member';
import {
  RevisionMetadataContribution,
  RevisionMetadataExcerpt,
} from '../models/revision';
import { Roaster } from '../models/roaster';

@Injectable({
  providedIn: 'root',
})
export class RoastersFrameService {
  roasterId = signal<number | null>(null);
  roaster = signal<Roaster | null>(null);
  roasterRevisionHistory = signal<null | RevisionMetadataExcerpt[]>(null);

  reset() {
    this.roaster.set(null);
    this.roasterRevisionHistory.set(null);
  }
}
