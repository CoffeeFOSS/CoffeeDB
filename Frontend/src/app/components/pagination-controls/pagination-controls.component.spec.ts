import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoadingService } from '../../services/loading.service';
import { By } from '@angular/platform-browser';
import { ComponentRef, DebugElement } from '@angular/core';
import { PaginationControlsComponent } from './pagination-controls.component';
import { QUERY_PARAMS } from '../../constants/query.constants';

describe('PaginationControlsComponent', () => {
  let component: PaginationControlsComponent<any>;
  let ref: ComponentRef<PaginationControlsComponent<any>>;
  let fixture: ComponentFixture<PaginationControlsComponent<any>>;
  let loadingService: LoadingService;
  let element: DebugElement;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [PaginationControlsComponent],
      providers: [LoadingService],
    }).compileComponents();

    loadingService = TestBed.inject(LoadingService);
    fixture = TestBed.createComponent(PaginationControlsComponent);
    component = fixture.componentInstance;
    ref = fixture.componentRef;

    ref.setInput('loadingId', 'test');
    ref.setInput('page', 1);
    // ref.setOutput('pageChange', 1);
    ref.setInput('pageSize', 5);
    // ref.setOutput('pageSizeChange', 1);

    ref.setInput('paginatedResult', {
      items: [1, 2, 3, 4, 5, 6, 7], // These would be specific types instead of just numbers
      pagination: {
        currentPage: 1,
        itemsPerPage: 5,
        totalItems: 7,
        totalPages: 2,
      },
    });

    fixture.detectChanges();
  });

  it('creates component', () => {
    expect(component).toBeTruthy;
  });

  it('set initial data', () => {});

  describe('pagination functionality', () => {
    beforeEach(() => {});

    it('renders pagination controls container', () => {
      const container = fixture.debugElement;
      expect(container).toBeTruthy();
    });

    it('renders prev button', () => {
      const prevButton = fixture.debugElement.query(
        By.css('[data-testid="prev-button"]'),
      );
      expect(prevButton).toBeTruthy();
      expect(prevButton.nativeElement.disabled).toBe(true);
    });

    it('renders next button', () => {
      const nextButton = fixture.debugElement.query(
        By.css('[data-testid="next-button"]'),
      );
      expect(nextButton).toBeTruthy();
      expect(nextButton.nativeElement.disabled).toBe(false);
    });

    it('turn pages', () => {
      const nextButton = fixture.debugElement.query(
        By.css('[data-testid="next-button"]'),
      );
      const prevButton = fixture.debugElement.query(
        By.css('[data-testid="prev-button"]'),
      );

      let emittedPage = 0;
      component.pageChange.subscribe((p) => (emittedPage = p));

      expect(nextButton.nativeElement.disabled).toBe(false);
      expect(prevButton.nativeElement.disabled).toBe(true);

      nextButton.nativeElement.click();
      fixture.detectChanges();
      expect(emittedPage).toBe(2);

      ref.setInput('page', 2);
      fixture.detectChanges();
      expect(prevButton.nativeElement.disabled).toBe(false);

      prevButton.nativeElement.click();
      fixture.detectChanges();
      expect(emittedPage).toBe(1);

      ref.setInput('page', 1);
      fixture.detectChanges();
      expect(nextButton.nativeElement.disabled).toBe(false);
    });

    it('should not render page size selector', () => {
      const pageSizeSelect = fixture.debugElement.query(
        By.css('[data-testid="page-size-selector"]'),
      );
      expect(pageSizeSelect).toBeFalsy();
    });

    describe('page size change functionality', () => {
      beforeEach(() => {
        ref.setInput('allowPageSizeEdit', true);
        fixture.detectChanges();
      });

      it('renders page size selector', () => {
        const pageSizeSelect = fixture.debugElement.query(
          By.css('[data-testid="page-size-selector"]'),
        );
        expect(pageSizeSelect).toBeTruthy();
      });

      it('changes page size', () => {
        const newPageSize = 7;
        let emittedPageSize: number | undefined;
        component.pageSizeChange.subscribe((size) => (emittedPageSize = size));

        const input = fixture.debugElement.query(
          By.css('[data-testid="page-size-selector"] input'),
        ).nativeElement as HTMLInputElement;

        const button = fixture.debugElement.query(
          By.css('[data-testid="page-size-selector"] button'),
        ).nativeElement as HTMLButtonElement;

        input.value = newPageSize.toString();
        input.dispatchEvent(new Event('input'));
        fixture.detectChanges();

        button.click();
        fixture.detectChanges();

        expect(emittedPageSize).toBe(newPageSize);
      });
    });
  });
});
