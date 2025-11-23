import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  RevisionMetadataWithEntityIdentifier,
  UserRevisionSearchParams,
} from '../models/revision';
import { GenericSearchParams } from '../models/pagination';
import { getHttpParams } from '../utils/params.utils';

@Injectable({
  providedIn: 'root',
})
export class RevisionsService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getUserRevisionContributions(
    searchParams: GenericSearchParams,
    userId: number,
  ) {
    return this.http.get<RevisionMetadataWithEntityIdentifier[]>(
      `${this.baseUrl}revisions/committed/${userId}`,
      {
        observe: 'response',
        params: getHttpParams(searchParams),
      },
    );
  }

  getUserRevisions(searchParams: UserRevisionSearchParams, userId: number) {
    return this.http.get<RevisionMetadataWithEntityIdentifier[]>(
      `${this.baseUrl}users/${userId}/revisions`,
      {
        observe: 'response',
        params: getHttpParams(searchParams),
      },
    );
  }
}
