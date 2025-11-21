import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { RevisionMetadataWithEntityIdentifier } from '../models/revision';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class RevisionsService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getUserRevisionContributions(userId: number) {
    // TODO: Add Pagination Params
    return this.http.get<RevisionMetadataWithEntityIdentifier[]>(
      `${this.baseUrl}revisions/committed/${userId}`,
    );
  }

  getUserPendingRevisions(userId: number) {
    // TODO: Add Pagination Params
    return this.http.get<RevisionMetadataWithEntityIdentifier[]>(
      `${this.baseUrl}revisions/pending?userId=${userId}`,
    );
  }
}
