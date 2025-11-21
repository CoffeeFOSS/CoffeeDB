import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  CreateRoasterDto,
  Roaster,
  RoasterRevisionDiff,
  RoasterRevisionSnapshot,
  RoasterSearchParams,
  UpdateRoasterDto,
} from '../models/roaster';
import { getHttpParams } from '../utils/params.utils';
import {
  RevisionMetadataContribution,
  RevisionMetadataExcerpt,
} from '../models/revision';

@Injectable({
  providedIn: 'root',
})
export class RevisionsService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getUserRevisionContributions(userId: number) {
    // TODO: Add Pagination Params
    return this.http.get<RevisionMetadataContribution[]>(
      `${this.baseUrl}revisions/committed/${userId}`,
    );
  }
}
