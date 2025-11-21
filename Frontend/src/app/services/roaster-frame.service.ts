import { Injectable, signal } from '@angular/core';
import { RevisionMetadataExcerpt } from '../models/revision';
import { Roaster } from '../models/roaster';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class RoastersFrameService {
  roasterId = signal<number | null>(null);
  roaster = signal<Roaster | null>(null);

  reset() {
    this.roaster.set(null);
  }
}
