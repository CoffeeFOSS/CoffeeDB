import { Injectable, signal } from '@angular/core';
import { Member } from '../models/member';
import { RevisionMetadataContribution } from '../models/revision';

@Injectable({
  providedIn: 'root',
})
export class UserFrameService {
  username = signal<string | null>(null);
  user = signal<Member | null>(null);
  userContributions = signal<RevisionMetadataContribution[] | null>(null); // setting it null instead of [] first to avoid repeated calls
}
