import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RevisionDiffRowComponent } from './revision-diff-row.component';

describe('RevisionDiffRowComponent', () => {
  let component: RevisionDiffRowComponent;
  let fixture: ComponentFixture<RevisionDiffRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RevisionDiffRowComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RevisionDiffRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
